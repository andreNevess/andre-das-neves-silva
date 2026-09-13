import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Solicitacoes internas",
  description: "Controle de solicitacoes internas de suporte"
};

export default function RootLayout({
  children
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR">
      <body>{children}</body>
    </html>
  );
}
