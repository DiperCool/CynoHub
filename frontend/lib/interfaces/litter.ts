export enum LitterStatus {
  Draft = 'Draft',
  Submitted = 'Submitted',
  Approved = 'Approved',
  Published = 'Published',
}

export interface LitterDto {
  id: string;
  breederId: string;
  status: LitterStatus;
  createdAt: string;
}

export interface PaginationMeta {
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface LitterDtoPagedResult {
  data: LitterDto[];
  pagination: PaginationMeta;
}
