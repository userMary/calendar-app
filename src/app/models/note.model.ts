// src/app/models/note.model.ts
export interface Note {
    id?: number;              // приходит от backend
    date: string;             // хранить ISO строкой, например "2025-08-09T12:00:00Z"
    title: string;
    description: string;
    color: 'white' | 'yellow' | 'green' | 'red' | 'purple' | 'blue' | string;
    imageUrl?: string;
    userId: number;
    // user?: Partial<User>; // опционально, если backend возвращает вложенного User
  }
  