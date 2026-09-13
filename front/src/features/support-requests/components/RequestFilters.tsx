"use client";

import { RefreshCw, Search } from "lucide-react";
import { priorities, priorityLabels, statuses, statusLabels } from "../lib/labels";
import type { RequestPriority, RequestStatus } from "../lib/types";
import { Dropdown, type DropdownOption } from "./Dropdown";

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

const statusOptions: DropdownOption<RequestStatus | "">[] = [
  { value: "", label: "Todos" },
  ...statuses.map((status) => ({
    value: status,
    label: statusLabels[status]
  }))
];

const priorityOptions: DropdownOption<RequestPriority | "">[] = [
  { value: "", label: "Todas" },
  ...priorities.map((priority) => ({
    value: priority,
    label: priorityLabels[priority]
  }))
];

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
        <Dropdown
          id="status"
          options={statusOptions}
          value={filters.status}
          onChange={(status) =>
            onChange({
              ...filters,
              status
            })
          }
        />
      </div>

      <div className="field">
        <label htmlFor="priority">Prioridade</label>
        <Dropdown
          id="priority"
          options={priorityOptions}
          value={filters.priority}
          onChange={(priority) =>
            onChange({
              ...filters,
              priority
            })
          }
        />
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
