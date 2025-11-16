import { Injectable } from '@angular/core';
import { HttpService } from '../@backend/services/http.service';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

export interface User {
  email: string;
  username: string;
}

export interface ApiResponse<T = any> {
  success: boolean;
  message?: string;
  data?: T;
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpService, private router: Router) { }

  public logout(): void {
    this.http.logoutUser();
  }

  public async getUser(): Promise<User | null> {
    try {
      const user = await firstValueFrom(this.http.getUser());
      return user as User;
    } catch (err) {
      return null;
    }
  }

  public async login(email: string, password: string): Promise<ApiResponse> {
    try {
      const data = await firstValueFrom(this.http.loginUser(email, password));
      return { success: true, data };
    } catch (err: any) {
      return { success: false, message: err?.message || 'Login failed' };
    }
  }

  public async loginVulnerable(email: string, password: string): Promise<ApiResponse> {
    try {
      const data = await firstValueFrom(this.http.loginUserVuln(email, password));
      return { success: true, data };
    } catch (err: any) {
      return { success: false, message: err?.message || 'Vulnerable login failed' };
    }
  }

  public async register(
    email: string,
    username: string,
    password: string
  ): Promise<ApiResponse> {
    try {
      const data = await firstValueFrom(
        this.http.registerUser(email, username, password)
      );
      return { success: true, data };
    } catch (err: any) {
      return { success: false, message: err?.message || 'Registration failed' };
    }
  }
}
