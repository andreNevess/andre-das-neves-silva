"use client";

import { AlertCircle, Eye, LoaderCircle } from "lucide-react";
import { formatDateTime } from "../lib/labels";
import type { SupportRequest } from "../lib/types";
import { StatusBadge } from "./StatusBadge";

type RequestListProps = {
  requests: SupportRequest[];
  selectedId?: string;
  isLoading: boolean;
  error?: string;
  onSelect: (id: string) => void;
};

export function RequestList({
  requests,
  selectedId,
  isLoading,
  error,
  onSelect
}: RequestListProps) {
  if (isLoading) {
    return (
      <div className="loadingState" role="status">
        <LoaderCircle aria-hidden="true" />
        <span>Carregando solicitações...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="errorState" role="alert">
        <AlertCircle aria-hidden="true" />
        <span>{error}</span>
      </div>
    );
  }

  if (requests.length === 0) {
    return (
      <div className="emptyState">
        <span>Nenhuma solicitação encontrada.</span>
      </div>
    );
  }

  return (
    <div className="requestList">
      {requests.map((request) => (
        <button
          className={`requestRow ${selectedId === request.id ? "active" : ""}`}
          key={request.id}
          type="button"
          onClick={() => onSelect(request.id)}
        >
          <div>
            <p className="rowTitle">
              <Eye aria-hidden="true" size={18} />
              <span>{request.title}</span>
            </p>
            <div className="rowMeta">
              <span>{request.requester}</span>
              <span>{formatDateTime(request.createdAtUtc)}</span>
            </div>
          </div>
          <div className="badges">
            <StatusBadge status={request.status} />
            <StatusBadge priority={request.priority} />
          </div>
        </button>
      ))}
    </div>
  );
}
