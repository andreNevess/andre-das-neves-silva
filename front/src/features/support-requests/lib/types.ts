export type RequestPriority = "Low" | "Medium" | "High";

export type RequestStatus = "Open" | "InProgress" | "Completed";

export type SupportRequest = {
  id: string;
  title: string;
  description: string;
  requester: string;
  priority: RequestPriority;
  status: RequestStatus;
  createdAtUtc: string;
  completedAtUtc: string | null;
};

export type PagedResult<T> = {
  items: T[];
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
};

export type ListSupportRequestsParams = {
  status?: RequestStatus;
  priority?: RequestPriority;
  search?: string;
  pageNumber: number;
  pageSize: number;
};

export type CreateSupportRequestPayload = {
  title: string;
  description: string;
  requester: string;
  priority: RequestPriority;
};

export type UpdateSupportRequestPayload = {
  priority: RequestPriority;
  status: RequestStatus;
};
