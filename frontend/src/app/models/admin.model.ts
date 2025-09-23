// src/app/models/admin.model.ts
export interface Admin {
  id?: number;
  email: string;
  password?: string; // фронт отправляет обычный пароль
  name?: string;
}