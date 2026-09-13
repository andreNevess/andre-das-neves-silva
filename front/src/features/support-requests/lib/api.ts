import type {
  CreateSupportRequestPayload,
  ListSupportRequestsParams,
  PagedResult,
  SupportRequest,
  UpdateSupportRequestPayload
} from "./types";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5132";

type ProblemDetails = {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
};

export class ApiError extends Error {
  readonly messages: string[];

  constructor(messages: string[]) {
    super(messages[0] ?? "Não foi possível completar a operação.");
    this.messages = messages;
  }
}

export async function listSupportRequests(
  params: ListSupportRequestsParams,
  signal?: AbortSignal
): Promise<PagedResult<SupportRequest>> {
  const searchParams = new URLSearchParams({
    pageNumber: String(params.pageNumber),
    pageSize: String(params.pageSize)
  });

  if (params.status) {
    searchParams.set("status", params.status);
  }

  if (params.priority) {
    searchParams.set("priority", params.priority);
  }

  if (params.search?.trim()) {
    searchParams.set("search", params.search.trim());
  }

  return request<PagedResult<SupportRequest>>(`/api/requests?${searchParams}`, {
    signal
  });
}

export async function getSupportRequest(
  id: string,
  signal?: AbortSignal
): Promise<SupportRequest> {
  return request<SupportRequest>(`/api/requests/${id}`, { signal });
}

export async function createSupportRequest(
  payload: CreateSupportRequestPayload
): Promise<SupportRequest> {
  return request<SupportRequest>("/api/requests", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export async function updateSupportRequest(
  id: string,
  payload: UpdateSupportRequestPayload
): Promise<SupportRequest> {
  return request<SupportRequest>(`/api/requests/${id}`, {
    method: "PATCH",
    body: JSON.stringify(payload)
  });
}

export async function deleteSupportRequest(id: string): Promise<void> {
  await request<void>(`/api/requests/${id}`, {
    method: "DELETE"
  });
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...init.headers
    }
  });

  if (!response.ok) {
    throw new ApiError(await readMessages(response));
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function readMessages(response: Response): Promise<string[]> {
  try {
    const problem = (await response.json()) as ProblemDetails;
    const validationMessages = problem.errors
      ? Object.values(problem.errors).flat()
      : [];

    return [
      ...validationMessages,
      problem.detail,
      problem.title
    ].filter((message): message is string => Boolean(message));
  } catch {
    return ["Não foi possível completar a operação."];
  }
}
