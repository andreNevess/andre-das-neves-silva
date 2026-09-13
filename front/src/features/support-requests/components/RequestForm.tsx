"use client";

import { LoaderCircle, Plus } from "lucide-react";
import { FormEvent, useState } from "react";
import { priorities, priorityLabels } from "../lib/labels";
import type { CreateSupportRequestPayload, RequestPriority } from "../lib/types";
import { Dropdown, type DropdownOption } from "./Dropdown";

type RequestFormProps = {
  isSubmitting: boolean;
  onSubmit: (payload: CreateSupportRequestPayload) => Promise<void>;
};

type FormErrors = Partial<Record<keyof CreateSupportRequestPayload, string>>;

const initialForm: CreateSupportRequestPayload = {
  title: "",
  description: "",
  requester: "",
  priority: "Medium"
};

const priorityOptions: DropdownOption<RequestPriority>[] = priorities.map((priority) => ({
  value: priority,
  label: priorityLabels[priority]
}));

export function RequestForm({ isSubmitting, onSubmit }: RequestFormProps) {
  const [form, setForm] = useState<CreateSupportRequestPayload>(initialForm);
  const [errors, setErrors] = useState<FormErrors>({});

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const nextErrors = validate(form);
    setErrors(nextErrors);

    if (Object.keys(nextErrors).length > 0) {
      return;
    }

    await onSubmit({
      ...form,
      title: form.title.trim(),
      description: form.description.trim(),
      requester: form.requester.trim()
    });

    setForm(initialForm);
    setErrors({});
  }

  return (
    <form className="form" onSubmit={handleSubmit} noValidate>
      <div className="field">
        <label htmlFor="title">Título</label>
        <input
          aria-describedby={errors.title ? "title-error" : undefined}
          aria-invalid={Boolean(errors.title)}
          className="input"
          id="title"
          maxLength={120}
          value={form.title}
          onChange={(event) => setForm({ ...form, title: event.target.value })}
        />
        {errors.title ? (
          <span className="formError" id="title-error">
            {errors.title}
          </span>
        ) : null}
      </div>

      <div className="field">
        <label htmlFor="requester">Solicitante</label>
        <input
          aria-describedby={errors.requester ? "requester-error" : undefined}
          aria-invalid={Boolean(errors.requester)}
          className="input"
          id="requester"
          maxLength={120}
          value={form.requester}
          onChange={(event) => setForm({ ...form, requester: event.target.value })}
        />
        {errors.requester ? (
          <span className="formError" id="requester-error">
            {errors.requester}
          </span>
        ) : null}
      </div>

      <div className="field">
        <label htmlFor="newPriority">Prioridade</label>
        <Dropdown
          id="newPriority"
          options={priorityOptions}
          value={form.priority}
          onChange={(priority) =>
            setForm({
              ...form,
              priority
            })
          }
        />
      </div>

      <div className="field">
        <label htmlFor="description">Descrição</label>
        <textarea
          aria-describedby={errors.description ? "description-error" : undefined}
          aria-invalid={Boolean(errors.description)}
          className="textarea"
          id="description"
          maxLength={2000}
          value={form.description}
          onChange={(event) =>
            setForm({ ...form, description: event.target.value })
          }
        />
        {errors.description ? (
          <span className="formError" id="description-error">
            {errors.description}
          </span>
        ) : null}
      </div>

      <div className="formActions">
        <button className="button" type="submit" disabled={isSubmitting}>
          {isSubmitting ? (
            <LoaderCircle className="savingIcon" aria-hidden="true" />
          ) : (
            <Plus aria-hidden="true" />
          )}
          Registrar solicitação
        </button>
      </div>
    </form>
  );
}

function validate(form: CreateSupportRequestPayload): FormErrors {
  const errors: FormErrors = {};

  if (!form.title.trim()) {
    errors.title = "Informe o título.";
  }

  if (!form.requester.trim()) {
    errors.requester = "Informe o solicitante.";
  }

  if (!form.description.trim()) {
    errors.description = "Informe a descrição.";
  }

  return errors;
}
