# Project TODOs

This file tracks technical debt and future architectural improvements for CynoHub.

## Testing
- [ ] **Integration Tests**: Implement integration tests using `WebApplicationFactory` and a real database (e.g., SQLite in-memory or Testcontainers). This is crucial to verify that the API controllers, DI container, EF Core configurations, and Global Exception Handler all work correctly together.

## Architecture & Reliability
- [x] **Transactional Outbox Pattern**: Implement the Outbox pattern for `INotificationService`. Currently, notifications are sent directly at the end of the `PublishAsync` use case. This should be changed to write an `OutboxMessage` to the database inside the same transaction as the domain changes, and then processed by a background worker to guarantee delivery.
- [x] **Database Retry Logic (Polly)**: Wrap high-concurrency database operations (like `PublishAsync`) with a transient fault handling library like `Polly`. This will automatically retry the transaction when an optimistic concurrency conflict (`DbUpdateConcurrencyException`) occurs, improving user experience instead of throwing a `409 Conflict` directly to the client.

## Security
- [ ] **Authentication & Authorization**: Replace the stub `X-Breeder-Id` header implementation with real JWT Bearer token authentication. Ensure `breederId` is securely extracted from `User.Claims` in the controllers.

