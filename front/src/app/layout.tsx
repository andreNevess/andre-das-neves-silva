import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Solicitações internas",
  description: "Controle de solicitações internas de suporte"
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
