"use client";

import { ChevronDown } from "lucide-react";
import { useEffect, useId, useRef, useState } from "react";

export type DropdownOption<TValue extends string> = {
  value: TValue;
  label: string;
  disabled?: boolean;
};

type DropdownProps<TValue extends string> = {
  id: string;
  value: TValue;
  options: DropdownOption<TValue>[];
  disabled?: boolean;
  onChange: (value: TValue) => void;
};

export function Dropdown<TValue extends string>({
  id,
  value,
  options,
  disabled = false,
  onChange
}: DropdownProps<TValue>) {
  const [isOpen, setIsOpen] = useState(false);
  const listboxId = useId();
  const containerRef = useRef<HTMLDivElement>(null);
  const selectedOption = options.find((option) => option.value === value);

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    function handlePointerDown(event: PointerEvent) {
      if (!containerRef.current?.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setIsOpen(false);
      }
    }

    document.addEventListener("pointerdown", handlePointerDown);
    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("pointerdown", handlePointerDown);
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [isOpen]);

  function handleSelect(option: DropdownOption<TValue>) {
    if (option.disabled) {
      return;
    }

    onChange(option.value);
    setIsOpen(false);
  }

  return (
    <div className="dropdown" ref={containerRef}>
      <button
        aria-controls={isOpen ? listboxId : undefined}
        aria-expanded={isOpen}
        aria-haspopup="listbox"
        className="dropdownTrigger"
        disabled={disabled}
        id={id}
        type="button"
        onClick={() => setIsOpen((current) => !current)}
      >
        <span>{selectedOption?.label ?? "Selecione"}</span>
        <ChevronDown aria-hidden="true" />
      </button>

      {isOpen ? (
        <div className="dropdownMenu" id={listboxId} role="listbox">
          {options.map((option) => (
            <button
              aria-selected={option.value === value}
              className="dropdownOption"
              disabled={option.disabled}
              key={option.value}
              role="option"
              type="button"
              onClick={() => handleSelect(option)}
            >
              {option.label}
            </button>
          ))}
        </div>
      ) : null}
    </div>
  );
}
