import { priorityLabels, statusLabels } from "../lib/labels";
import type { RequestPriority, RequestStatus } from "../lib/types";

type StatusBadgeProps = {
  status?: RequestStatus;
  priority?: RequestPriority;
};

export function StatusBadge({ status, priority }: StatusBadgeProps) {
  if (status) {
    return (
      <span className={`badge status${status}`}>
        {statusLabels[status]}
      </span>
    );
  }

  if (priority) {
    return (
      <span className={`badge priority${priority}`}>
        {priorityLabels[priority]}
      </span>
    );
  }

  return null;
}
