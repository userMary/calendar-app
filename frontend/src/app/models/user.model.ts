// src/app/models/user.model.ts
import { Note } from './note.model';

export interface User {
  id?: number;
  email: string;
  passwordHash: string;
  name?: string;
  notes?: Note[];
}
