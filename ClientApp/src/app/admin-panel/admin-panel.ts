import { Component, OnInit } from '@angular/core';
import { KENDO_GRID } from '@progress/kendo-angular-grid';
import { KENDO_LABELS } from '@progress/kendo-angular-label';
import { KENDO_INPUTS } from '@progress/kendo-angular-inputs';
import { KENDO_BUTTONS } from '@progress/kendo-angular-buttons';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../user-service/user';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { User } from '../models/user';
import { securityLockOutlineIcon, SVGIcon } from '@progress/kendo-svg-icons';
import { Header } from "../header/header";

@Component({
  selector: 'app-admin-panel',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    KENDO_GRID,
    KENDO_LABELS,
    KENDO_INPUTS,
    KENDO_BUTTONS,
    Header
],
  templateUrl: './admin-panel.html',
  styleUrls: ['./admin-panel.css'],
})
export class AdminPanel implements OnInit {
  public vulnerableMode: boolean = false;
  public searchQuery: string = '';
  public users: User[] = [];
  public username = 'Admin';
  public securitySVG: SVGIcon = securityLockOutlineIcon;

  constructor(private userService: UserService, private router: Router) {}

  public async ngOnInit(): Promise<void> {
    const user = await this.userService.getUser();
    if (user) {
      this.username = user.username || user.email;
    }
    this.fetchUsers();
  }

  public onSearchChange(): void {
    this.fetchUsers();
  }

  public async fetchUsers(): Promise<void> {
    if (this.searchQuery !== undefined) {
      const result = await this.userService.fetchUsers(this.searchQuery, this.vulnerableMode);
      this.users = result ? result.users : [];
    }
  }

  public logout(): void {
    this.userService.logout();
    this.router.navigate(['/signup']);
  }

  public forumNavigate(): void {
    this.router.navigate(['/forum']); 
  }
}
