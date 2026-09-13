"use client";

import { RefreshCw, Search } from "lucide-react";
import { priorities, priorityLabels, statuses, statusLabels } from "../lib/labels";
import type { RequestPriority, RequestStatus } from "../lib/types";

export type RequestFilterState = {
  search: string;
  status: RequestStatus | "";
  priority: RequestPriority | "";
};

type RequestFiltersProps = {
  filters: RequestFilterState;
  onChange: (filters: RequestFilterState) => void;
  onReset: () => void;
};

export function RequestFilters({ filters, onChange, onReset }: RequestFiltersProps) {
  return (
    <div className="filters" aria-label="Filtros de solicitacoes">
      <div className="field">
        <label htmlFor="search">Pesquisa</label>
        <div className="inputShell">
          <Search aria-hidden="true" />
          <input
            className="input withIcon"
            id="search"
            value={filters.search}
            onChange={(event) =>
              onChange({ ...filters, search: event.target.value })
            }
            placeholder="Titulo ou solicitante"
          />
        </div>
      </div>

      <div className="field">
        <label htmlFor="status">Status</label>
        <select
          className="select"
          id="status"
          value={filters.status}
          onChange={(event) =>
            onChange({
              ...filters,
              status: event.target.value as RequestStatus | ""
            })
          }
        >
          <option value="">Todos</option>
          {statuses.map((status) => (
            <option key={status} value={status}>
              {statusLabels[status]}
            </option>
          ))}
        </select>
      </div>

      <div className="field">
        <label htmlFor="priority">Prioridade</label>
        <select
          className="select"
          id="priority"
          value={filters.priority}
          onChange={(event) =>
            onChange({
              ...filters,
              priority: event.target.value as RequestPriority | ""
            })
          }
        >
          <option value="">Todas</option>
          {priorities.map((priority) => (
            <option key={priority} value={priority}>
              {priorityLabels[priority]}
            </option>
          ))}
        </select>
      </div>

      <div className="field">
        <label aria-hidden="true">&nbsp;</label>
        <button
          className="button secondary iconOnly"
          type="button"
          title="Limpar filtros"
          aria-label="Limpar filtros"
          onClick={onReset}
        >
          <RefreshCw aria-hidden="true" />
        </button>
      </div>
    </div>
  );
}
