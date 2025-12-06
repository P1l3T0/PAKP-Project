import { Component, OnInit } from '@angular/core';
import { UserService } from '../user-service/user';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { KENDO_BUTTON } from '@progress/kendo-angular-buttons';

@Component({
  selector: 'app-header',
  imports: [KENDO_BUTTON, RouterLink, RouterLinkActive],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header implements OnInit {
  public username: string = 'User';
  public userId: number = 0;

  constructor(private userService: UserService, private router: Router) {}

  public async ngOnInit(): Promise<void> {
    const user = await this.userService.getUser();
    if (user) {
      this.userId = user.id || 0;
      this.username = user.username || user.email;
    }
  }

  public logout(): void {
    this.userService.logout();
    this.router.navigate(['/signup']);
  }
}
