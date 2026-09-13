import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { RequestsPage } from "./RequestsPage";
import type {
  PagedResult,
  RequestPriority,
  RequestStatus,
  SupportRequest
} from "../lib/types";

let requests: SupportRequest[];
let shouldFailList = false;

const createdRequest: SupportRequest = {
  id: "0f708416-6f3a-4ac5-b60c-b308a7fb6a93",
  title: "Monitor adicional",
  description: "Analista precisa de um segundo monitor.",
  requester: "Bruno Costa",
  priority: "Medium",
  status: "Open",
  createdAtUtc: "2026-09-12T13:00:00Z",
  completedAtUtc: null
};

function buildRequest(
  id: string,
  title: string,
  requester: string,
  priority: RequestPriority,
  status: RequestStatus,
  createdAtUtc: string
): SupportRequest {
  return {
    id,
    title,
    description: `${title} - descrição detalhada.`,
    requester,
    priority,
    status,
    createdAtUtc,
    completedAtUtc: status === "Completed" ? "2026-09-12T16:00:00Z" : null
  };
}

function createPage(url: URL): PagedResult<SupportRequest> {
  const status = url.searchParams.get("status");
  const priority = url.searchParams.get("priority");
  const search = url.searchParams.get("search")?.toLowerCase();
  const pageNumber = Number(url.searchParams.get("pageNumber") ?? "1");
  const pageSize = Number(url.searchParams.get("pageSize") ?? "5");

  const filtered = requests
    .filter((request) => !status || request.status === status)
    .filter((request) => !priority || request.priority === priority)
    .filter((request) => {
      if (!search) {
        return true;
      }

      return (
        request.title.toLowerCase().includes(search) ||
        request.requester.toLowerCase().includes(search)
      );
    })
    .sort((left, right) => right.createdAtUtc.localeCompare(left.createdAtUtc));

  const start = (pageNumber - 1) * pageSize;
  const items = filtered.slice(start, start + pageSize);

  return {
    items,
    totalItems: filtered.length,
    pageNumber,
    pageSize,
    totalPages: Math.ceil(filtered.length / pageSize)
  };
}

function jsonResponse(body: unknown, status = 200) {
  return new Response(JSON.stringify(body), {
    status,
    headers: {
      "Content-Type": "application/json"
    }
  });
}

function setupFetch() {
  const fetchMock = vi.fn(async (input: RequestInfo | URL, init?: RequestInit) => {
    const url = new URL(String(input));
    const method = init?.method ?? "GET";

    if (url.pathname === "/api/requests" && method === "GET") {
      if (shouldFailList) {
        return jsonResponse(
          {
            title: "Erro inesperado",
            detail: "Não foi possível carregar as solicitações."
          },
          500
        );
      }

      return jsonResponse(createPage(url));
    }

    if (url.pathname === "/api/requests" && method === "POST") {
      const payload = JSON.parse(String(init?.body)) as {
        title: string;
        description: string;
        requester: string;
        priority: RequestPriority;
      };

      const nextRequest = {
        ...createdRequest,
        ...payload
      };

      requests = [nextRequest, ...requests];

      return jsonResponse(nextRequest, 201);
    }

    const requestId = url.pathname.startsWith("/api/requests/")
      ? url.pathname.replace("/api/requests/", "")
      : undefined;

    if (requestId && method === "GET") {
      const request = requests.find((item) => item.id === requestId);

      return request
        ? jsonResponse(request)
        : jsonResponse({ detail: "Solicitação não encontrada." }, 404);
    }

    if (requestId && method === "PATCH") {
      const payload = JSON.parse(String(init?.body)) as {
        priority: RequestPriority;
        status: RequestStatus;
      };

      const request = requests.find((item) => item.id === requestId);

      if (!request) {
        return jsonResponse({ detail: "Solicitação não encontrada." }, 404);
      }

      const updated = {
        ...request,
        priority: payload.priority,
        status: payload.status,
        completedAtUtc:
          payload.status === "Completed" ? "2026-09-12T17:00:00Z" : null
      };

      requests = requests.map((item) => (item.id === updated.id ? updated : item));

      return jsonResponse(updated);
    }

    if (requestId && method === "DELETE") {
      requests = requests.filter((item) => item.id !== requestId);

      return new Response(null, { status: 204 });
    }

    throw new Error(`Unexpected request: ${method} ${url.pathname}`);
  });

  vi.stubGlobal("fetch", fetchMock);

  return fetchMock;
}

async function chooseDropdownOption(
  user: ReturnType<typeof userEvent.setup>,
  trigger: HTMLElement,
  optionName: string
) {
  await user.click(trigger);
  await user.click(screen.getByRole("option", { name: optionName }));
}

describe("RequestsPage", () => {
  beforeEach(() => {
    requests = [
      buildRequest(
        "5a6a222f-8b4b-4d15-b474-32d8f27f36f4",
        "VPN corporativa instavel",
        "Ana Silva",
        "High",
        "Open",
        "2026-09-12T12:00:00Z"
      ),
      buildRequest(
        "68cbad27-9097-4623-ad2a-7a18465589f1",
        "VPN sem acesso",
        "Carla Souza",
        "High",
        "Open",
        "2026-09-12T11:00:00Z"
      ),
      buildRequest(
        "1ab7607b-1550-4f3a-b18e-f2b829db5ef1",
        "VPN lenta",
        "Diego Lima",
        "High",
        "Open",
        "2026-09-12T10:00:00Z"
      ),
      buildRequest(
        "1a5e4c53-98a3-49a3-8e75-3a70eedf5f1d",
        "VPN erro intermitente",
        "Elisa Rocha",
        "High",
        "Open",
        "2026-09-12T09:00:00Z"
      ),
      buildRequest(
        "3d0b1031-caa7-4810-b740-455968c319f8",
        "VPN renovação de certificado",
        "Fabio Nunes",
        "High",
        "Open",
        "2026-09-12T08:00:00Z"
      ),
      buildRequest(
        "315ee758-a8b2-48f8-a1d5-af25b0148ea1",
        "VPN perfil externo",
        "Giovana Alves",
        "High",
        "Open",
        "2026-09-12T07:00:00Z"
      ),
      buildRequest(
        "c2054759-277f-4c83-a9b4-b1ca35ad83d4",
        "Impressora fiscal",
        "Henrique Dias",
        "Medium",
        "InProgress",
        "2026-09-12T06:00:00Z"
      )
    ];
    shouldFailList = false;
    vi.spyOn(window, "confirm").mockReturnValue(true);
    setupFetch();
  });

  afterEach(() => {
    vi.restoreAllMocks();
    vi.unstubAllGlobals();
  });

  it("lists requests and creates a new one from the main form", async () => {
    const user = userEvent.setup();
    render(<RequestsPage />);

    expect(await screen.findByText("VPN corporativa instavel")).toBeInTheDocument();

    await user.click(
      screen.getByRole("button", { name: /registrar solicitação/i })
    );

    expect(screen.getByText("Informe o título.")).toBeInTheDocument();
    expect(screen.getByText("Informe o solicitante.")).toBeInTheDocument();
    expect(screen.getByText("Informe a descrição.")).toBeInTheDocument();

    await user.type(screen.getByLabelText("Título"), createdRequest.title);
    await user.type(screen.getByLabelText("Solicitante"), createdRequest.requester);
    await user.type(screen.getByLabelText("Descrição"), createdRequest.description);

    await user.click(
      screen.getByRole("button", { name: /registrar solicitação/i })
    );

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(
        "http://localhost:5132/api/requests",
        expect.objectContaining({
          method: "POST"
        })
      );
    });

    expect(await screen.findByText("Solicitação registrada.")).toBeInTheDocument();
  });

  it("applies search, filters and pagination", async () => {
    const user = userEvent.setup();
    render(<RequestsPage />);

    expect(await screen.findByText("VPN corporativa instavel")).toBeInTheDocument();

    await user.type(screen.getByLabelText("Pesquisa"), "VPN");
    await chooseDropdownOption(user, screen.getByLabelText("Status"), "Aberta");
    await chooseDropdownOption(user, screen.getAllByLabelText("Prioridade")[0], "Alta");

    await waitFor(() => {
      const urls = vi
        .mocked(fetch)
        .mock.calls
        .map(([input]) => new URL(String(input)));

      expect(
        urls.some((url) =>
          url.searchParams.get("search") === "VPN" &&
          url.searchParams.get("status") === "Open" &&
          url.searchParams.get("priority") === "High"
        )
      ).toBe(true);
    });

    await user.click(screen.getByRole("button", { name: "Próxima página" }));

    await waitFor(() => {
      const urls = vi
        .mocked(fetch)
        .mock.calls
        .map(([input]) => new URL(String(input)));

      expect(
        urls.some((url) => url.searchParams.get("pageNumber") === "2")
      ).toBe(true);
    });
  });

  it("updates the selected request", async () => {
    const user = userEvent.setup();
    render(<RequestsPage />);

    await user.click(await screen.findByText("VPN corporativa instavel"));
    expect(await screen.findByText(/descrição detalhada/i)).toBeInTheDocument();

    await chooseDropdownOption(user, screen.getAllByLabelText("Prioridade").at(-1)!, "Média");
    await chooseDropdownOption(user, screen.getAllByLabelText("Status").at(-1)!, "Em andamento");
    await user.click(screen.getByRole("button", { name: /salvar alterações/i }));

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(
        "http://localhost:5132/api/requests/5a6a222f-8b4b-4d15-b474-32d8f27f36f4",
        expect.objectContaining({
          method: "PATCH"
        })
      );
    });

    expect(await screen.findByText("Solicitação atualizada.")).toBeInTheDocument();
  });

  it("deletes an open selected request after confirmation", async () => {
    const user = userEvent.setup();
    render(<RequestsPage />);

    await user.click(await screen.findByText("VPN corporativa instavel"));
    await user.click(await screen.findByRole("button", { name: /excluir/i }));

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(
        "http://localhost:5132/api/requests/5a6a222f-8b4b-4d15-b474-32d8f27f36f4",
        expect.objectContaining({
          method: "DELETE"
        })
      );
    });

    expect(await screen.findByText("Solicitação excluída.")).toBeInTheDocument();
  });

  it("shows empty and error states", async () => {
    requests = [];
    const { unmount } = render(<RequestsPage />);

    expect(await screen.findByText("Nenhuma solicitação encontrada.")).toBeInTheDocument();

    unmount();
    shouldFailList = true;
    render(<RequestsPage />);

    expect(
      await screen.findByText(/Não foi possível carregar as solicitações/i)
    ).toBeInTheDocument();
  });
});
