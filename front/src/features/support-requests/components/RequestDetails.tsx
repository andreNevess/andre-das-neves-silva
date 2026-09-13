"use client";

import { AlertCircle, LoaderCircle, Save, Trash2 } from "lucide-react";
import { FormEvent, useState } from "react";
import {
  formatDateTime,
  priorities,
  priorityLabels,
  statuses,
  statusLabels
} from "../lib/labels";
import type {
  RequestPriority,
  RequestStatus,
  SupportRequest,
  UpdateSupportRequestPayload
} from "../lib/types";
import { StatusBadge } from "./StatusBadge";

type RequestDetailsProps = {
  request?: SupportRequest;
  isLoading: boolean;
  error?: string;
  isSaving: boolean;
  onUpdate: (payload: UpdateSupportRequestPayload) => Promise<void>;
  onDelete: () => Promise<void>;
};

export function RequestDetails({
  request,
  isLoading,
  error,
  isSaving,
  onUpdate,
  onDelete
}: RequestDetailsProps) {
  const [priority, setPriority] = useState<RequestPriority>(
    request?.priority ?? "Medium"
  );
  const [status, setStatus] = useState<RequestStatus>(
    request?.status ?? "Open"
  );

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    await onUpdate({ priority, status });
  }

  if (isLoading) {
    return (
      <div className="loadingState" role="status">
        <LoaderCircle aria-hidden="true" />
        <span>Carregando detalhes...</span>
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

  if (!request) {
    return (
      <div className="emptyState">
        <span>Selecione uma solicitacao.</span>
      </div>
    );
  }

  const isCompleted = request.status === "Completed";

  return (
    <div className="details">
      <div>
        <div className="badges" style={{ justifyContent: "flex-start" }}>
          <StatusBadge status={request.status} />
          <StatusBadge priority={request.priority} />
        </div>
        <h3>{request.title}</h3>
        <p className="detailText">{request.description}</p>
      </div>

      <dl className="definitionGrid">
        <div>
          <dt>Solicitante</dt>
          <dd>{request.requester}</dd>
        </div>
        <div>
          <dt>Criacao</dt>
          <dd>{formatDateTime(request.createdAtUtc)}</dd>
        </div>
        <div>
          <dt>Conclusao</dt>
          <dd>{formatDateTime(request.completedAtUtc)}</dd>
        </div>
        <div>
          <dt>Identificador</dt>
          <dd>{request.id.slice(0, 8)}</dd>
        </div>
      </dl>

      <form className="form" onSubmit={handleSubmit}>
        <div className="field">
          <label htmlFor="detailPriority">Prioridade</label>
          <select
            className="select"
            id="detailPriority"
            value={priority}
            onChange={(event) => setPriority(event.target.value as RequestPriority)}
          >
            {priorities.map((item) => (
              <option key={item} value={item}>
                {priorityLabels[item]}
              </option>
            ))}
          </select>
        </div>

        <div className="field">
          <label htmlFor="detailStatus">Status</label>
          <select
            className="select"
            id="detailStatus"
            value={status}
            disabled={isCompleted}
            onChange={(event) => setStatus(event.target.value as RequestStatus)}
          >
            {statuses.map((item) => (
              <option
                disabled={isCompleted && item !== "Completed"}
                key={item}
                value={item}
              >
                {statusLabels[item]}
              </option>
            ))}
          </select>
        </div>

        <div className="detailActions">
          <button className="button" type="submit" disabled={isSaving}>
            {isSaving ? (
              <LoaderCircle className="savingIcon" aria-hidden="true" />
            ) : (
              <Save aria-hidden="true" />
            )}
            Salvar alteracoes
          </button>

          {request.status === "Open" ? (
            <button
              className="button danger"
              type="button"
              disabled={isSaving}
              onClick={onDelete}
            >
              <Trash2 aria-hidden="true" />
              Excluir
            </button>
          ) : null}
        </div>
      </form>
    </div>
  );
}
