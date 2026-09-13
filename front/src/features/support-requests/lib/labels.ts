import type { RequestPriority, RequestStatus } from "./types";

export const priorityLabels: Record<RequestPriority, string> = {
  Low: "Baixa",
  Medium: "Média",
  High: "Alta"
};

export const statusLabels: Record<RequestStatus, string> = {
  Open: "Aberta",
  InProgress: "Em andamento",
  Completed: "Concluída"
};

export const priorities: RequestPriority[] = ["Low", "Medium", "High"];

export const statuses: RequestStatus[] = ["Open", "InProgress", "Completed"];

export function formatDateTime(value: string | null): string {
  if (!value) {
    return "-";
  }

  return new Intl.DateTimeFormat("pt-BR", {
    dateStyle: "short",
    timeStyle: "short"
  }).format(new Date(value));
}
