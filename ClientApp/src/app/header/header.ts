import { Component, Input, OnInit } from '@angular/core';
import { UserService } from '../user-service/user';
import { Router } from '@angular/router';
import { KENDO_BUTTON } from '@progress/kendo-angular-buttons';

@Component({
  selector: 'app-header',
  imports: [KENDO_BUTTON],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header implements OnInit {
  @Input() public pageTitle: string = '';
  public username: string = 'User';

  constructor(private userService: UserService, private router: Router) {}

  public async ngOnInit(): Promise<void> {
    const user = await this.userService.getUser();
    if (user) {
      this.username = user.username || user.email;
    }
  }

  public logout(): void {
    this.userService.logout();
    this.router.navigate(['/signup']);
  }
}
