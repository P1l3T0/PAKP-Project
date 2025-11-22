import { User } from "./user";

export interface FetchUsersResponse {
  count: number;
  method: string;
  users: User[];
}