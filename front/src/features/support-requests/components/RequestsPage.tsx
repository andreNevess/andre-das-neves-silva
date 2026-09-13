"use client";

import { ChevronLeft, ChevronRight, ListChecks } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import {
  ApiError,
  createSupportRequest,
  deleteSupportRequest,
  getSupportRequest,
  listSupportRequests,
  updateSupportRequest
} from "../lib/api";
import type {
  CreateSupportRequestPayload,
  PagedResult,
  SupportRequest,
  UpdateSupportRequestPayload
} from "../lib/types";
import { RequestDetails } from "./RequestDetails";
import { RequestFilters, type RequestFilterState } from "./RequestFilters";
import { RequestForm } from "./RequestForm";
import { RequestList } from "./RequestList";

const pageSize = 5;
const initialFilters: RequestFilterState = {
  search: "",
  status: "",
  priority: ""
};

const emptyPage: PagedResult<SupportRequest> = {
  items: [],
  totalItems: 0,
  pageNumber: 1,
  pageSize,
  totalPages: 0
};

export function RequestsPage() {
  const [filters, setFilters] = useState<RequestFilterState>(initialFilters);
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [page, setPage] = useState<PagedResult<SupportRequest>>(emptyPage);
  const [selectedId, setSelectedId] = useState<string>();
  const [selectedRequest, setSelectedRequest] = useState<SupportRequest>();
  const [listError, setListError] = useState<string>();
  const [detailError, setDetailError] = useState<string>();
  const [isListLoading, setIsListLoading] = useState(true);
  const [isDetailLoading, setIsDetailLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  const activeParams = useMemo(
    () => ({
      pageNumber,
      pageSize,
      status: filters.status || undefined,
      priority: filters.priority || undefined,
      search: debouncedSearch || undefined
    }),
    [debouncedSearch, filters.priority, filters.status, pageNumber]
  );

  useEffect(() => {
    const timeout = window.setTimeout(() => {
      setDebouncedSearch(filters.search.trim());
    }, 300);

    return () => window.clearTimeout(timeout);
  }, [filters.search]);

  useEffect(() => {
    const controller = new AbortController();

    async function loadRequests() {
      setIsListLoading(true);
      setListError(undefined);

      try {
        const result = await listSupportRequests(activeParams, controller.signal);
        setPage(result);
      } catch (error) {
        if (controller.signal.aborted) {
          return;
        }

        setListError(readError(error));
      } finally {
        if (!controller.signal.aborted) {
          setIsListLoading(false);
        }
      }
    }

    loadRequests();

    return () => controller.abort();
  }, [activeParams, reloadKey]);

  useEffect(() => {
    if (!selectedId) {
      return;
    }

    const controller = new AbortController();
    const currentSelectedId = selectedId;

    async function loadDetails() {
      setIsDetailLoading(true);
      setDetailError(undefined);

      try {
        const result = await getSupportRequest(currentSelectedId, controller.signal);
        setSelectedRequest(result);
      } catch (error) {
        if (controller.signal.aborted) {
          return;
        }

        setDetailError(readError(error));
      } finally {
        if (!controller.signal.aborted) {
          setIsDetailLoading(false);
        }
      }
    }

    loadDetails();

    return () => controller.abort();
  }, [selectedId, reloadKey]);

  function handleFilterChange(nextFilters: RequestFilterState) {
    setFilters(nextFilters);
    setPageNumber(1);
  }

  function handleResetFilters() {
    setFilters(initialFilters);
    setDebouncedSearch("");
    setPageNumber(1);
  }

  async function handleCreate(payload: CreateSupportRequestPayload) {
    setIsSubmitting(true);
    setMessage("");

    try {
      const created = await createSupportRequest(payload);
      setSelectedId(created.id);
      setMessage("Solicitação registrada.");
      setReloadKey((value) => value + 1);
    } catch (error) {
      setMessage(readError(error));
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleUpdate(payload: UpdateSupportRequestPayload) {
    if (!selectedRequest) {
      return;
    }

    setIsSaving(true);
    setMessage("");

    try {
      const updated = await updateSupportRequest(selectedRequest.id, payload);
      setSelectedRequest(updated);
      setMessage("Solicitação atualizada.");
      setReloadKey((value) => value + 1);
    } catch (error) {
      setMessage(readError(error));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDelete() {
    if (!selectedRequest) {
      return;
    }

    const shouldDelete = window.confirm("Excluir esta solicitação?");

    if (!shouldDelete) {
      return;
    }

    setIsSaving(true);
    setMessage("");

    try {
      await deleteSupportRequest(selectedRequest.id);
      setSelectedId(undefined);
      setSelectedRequest(undefined);
      setMessage("Solicitação excluída.");
      setReloadKey((value) => value + 1);
    } catch (error) {
      setMessage(readError(error));
    } finally {
      setIsSaving(false);
    }
  }

  const canGoBack = page.pageNumber > 1;
  const canGoForward = page.totalPages > page.pageNumber;

  return (
    <main className="appShell">
      <header className="topBar">
        <div className="titleBlock">
          <h1>Solicitações internas</h1>
          <p>Registro e acompanhamento de suporte interno</p>
        </div>
        <div className="statusLine" aria-live="polite">
          {message}
        </div>
      </header>

      <div className="workspace">
        <section className="panel" aria-labelledby="requests-heading">
          <div className="panelHeader">
            <div>
              <h2 id="requests-heading">Solicitações</h2>
              <p>{page.totalItems} registro(s)</p>
            </div>
            <ListChecks aria-hidden="true" />
          </div>

          <RequestFilters
            filters={filters}
            onChange={handleFilterChange}
            onReset={handleResetFilters}
          />

          <RequestList
            requests={page.items}
            selectedId={selectedId}
            isLoading={isListLoading}
            error={listError}
            onSelect={setSelectedId}
          />

          <div className="panelFooter pagination">
            <span>
              Página {page.totalPages === 0 ? 0 : page.pageNumber} de{" "}
              {page.totalPages}
            </span>
            <div className="paginationControls">
              <button
                className="button secondary iconOnly"
                type="button"
                title="Página anterior"
                aria-label="Página anterior"
                disabled={!canGoBack}
                onClick={() => setPageNumber((value) => Math.max(1, value - 1))}
              >
                <ChevronLeft aria-hidden="true" />
              </button>
              <button
                className="button secondary iconOnly"
                type="button"
                title="Próxima página"
                aria-label="Próxima página"
                disabled={!canGoForward}
                onClick={() => setPageNumber((value) => value + 1)}
              >
                <ChevronRight aria-hidden="true" />
              </button>
            </div>
          </div>
        </section>

        <aside className="sideStack">
          <section className="panel" aria-labelledby="new-request-heading">
            <div className="panelHeader">
              <h2 id="new-request-heading">Nova solicitação</h2>
            </div>
            <div className="panelBody">
              <RequestForm isSubmitting={isSubmitting} onSubmit={handleCreate} />
            </div>
          </section>

          <section className="panel" aria-labelledby="details-heading">
            <div className="panelHeader">
              <h2 id="details-heading">Detalhes</h2>
            </div>
            <div className="panelBody">
              <RequestDetails
                key={`${selectedRequest?.id ?? "empty"}-${selectedRequest?.priority ?? ""}-${selectedRequest?.status ?? ""}`}
                request={selectedRequest}
                isLoading={isDetailLoading}
                error={detailError}
                isSaving={isSaving}
                onUpdate={handleUpdate}
                onDelete={handleDelete}
              />
            </div>
          </section>
        </aside>
      </div>
    </main>
  );
}

function readError(error: unknown): string {
  if (error instanceof ApiError) {
    return error.messages.join(" ");
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Não foi possível completar a operação.";
}
