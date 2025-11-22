import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { UserService } from './user-service/user';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private userService: UserService, private router: Router) {}

  async canActivate(): Promise<boolean> {
    const isAuthenticated = await this.userService.checkAuth();
    if (isAuthenticated) {
      return true;
    } else {
      this.router.navigate(['/signup']);
      return false;
    }
  }
}
