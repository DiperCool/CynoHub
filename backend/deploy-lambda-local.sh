#!/bin/bash
set -e

echo "Building Lambda..."
dotnet build src/CynoHub.NotificationLambda/CynoHub.NotificationLambda.csproj -c Release

echo "Publishing Lambda..."
dotnet publish src/CynoHub.NotificationLambda/CynoHub.NotificationLambda.csproj -c Release -o ./publish
cd publish
zip -r ../lambda.zip *
cd ..

echo "Creating/Updating Lambda function in LocalStack..."
# Delete if exists to make it easier to re-deploy
docker exec localstack-main awslocal lambda delete-function --function-name NotificationLambda --region us-east-1 || true

# We need to copy the zip to the container first, or mount it.
docker cp lambda.zip localstack-main:/tmp/lambda.zip

docker exec localstack-main awslocal lambda create-function \
    --function-name NotificationLambda \
    --runtime dotnet8 \
    --handler CynoHub.NotificationLambda::CynoHub.NotificationLambda.Function::FunctionHandler \
    --role arn:aws:iam::000000000000:role/lambda-role \
    --zip-file fileb:///tmp/lambda.zip \
    --region us-east-1

echo "Mapping SQS to Lambda..."
# Get Queue ARN
QUEUE_ARN=$(docker exec localstack-main awslocal sqs get-queue-attributes --queue-url http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/litter-events-queue --attribute-names QueueArn --query 'Attributes.QueueArn' --output text --region us-east-1)

# Check if mapping already exists to avoid duplicates
MAPPING_EXISTS=$(docker exec localstack-main awslocal lambda list-event-source-mappings --function-name NotificationLambda --event-source-arn $QUEUE_ARN --query 'EventSourceMappings[0].UUID' --output text --region us-east-1)

if [ "$MAPPING_EXISTS" == "None" ] || [ -z "$MAPPING_EXISTS" ]; then
    docker exec localstack-main awslocal lambda create-event-source-mapping \
        --function-name NotificationLambda \
        --batch-size 1 \
        --event-source-arn $QUEUE_ARN \
        --region us-east-1
    echo "Event source mapping created."
else
    echo "Event source mapping already exists ($MAPPING_EXISTS)."
fi

echo "Done! Run 'docker exec localstack-main awslocal logs tail /aws/lambda/NotificationLambda' to see the logs."
