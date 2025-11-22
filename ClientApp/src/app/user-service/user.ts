import { Injectable } from '@angular/core';
import { HttpService } from '../@backend/services/http.service';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';
import { User } from '../models/user';
import { FetchUsersResponse } from '../models/fetch-users-response';
import { ApiResponse } from '../models/api-response';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  public isLoggedInSubject = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpService, private router: Router) {}

  public async checkAuth(): Promise<boolean> {
    try {
      const user = await firstValueFrom(this.http.getUser());
      const isAuthenticated = !!user;
      this.isLoggedInSubject.next(isAuthenticated);
      return isAuthenticated;
    } catch {
      this.isLoggedInSubject.next(false);
      return false;
    }
  }

  public logout(): void {
    this.http.logoutUser();
    this.isLoggedInSubject.next(false);
  }

  public async getUser(): Promise<User | null> {
    try {
      const user = await firstValueFrom(this.http.getUser());
      return user as User;
    } catch (err) {
      console.error('Error fetching user:', err);
      return null;
    }
  }

  public async fetchUsers(searchQuery: string, vulnerable: boolean): Promise<FetchUsersResponse | null> {
    try {
      const users = await firstValueFrom(this.http.fetchUsers(searchQuery, vulnerable));
      return users as FetchUsersResponse;
    } catch (err) {
      console.error('Error fetching users:', err);
      return null;
    }
  }

  public async login(email: string, password: string, vulnerable: boolean): Promise<ApiResponse> {
    try {
      const data = await firstValueFrom(this.http.loginUser(email, password, vulnerable));
      this.isLoggedInSubject.next(true);
      return { success: true, data };
    } catch (err: any) {
      this.isLoggedInSubject.next(false);
      return { success: false, message: err?.error.error || 'Login failed' };
    }
  }

  public async register(email: string, username: string, password: string): Promise<ApiResponse> {
    try {
      const data = await firstValueFrom(this.http.registerUser(email, username, password));
      return { success: true, data };
    } catch (err: any) {
      return { success: false, message: err?.error.error || 'Registration failed' };
    }
  }
}
