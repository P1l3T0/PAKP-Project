import { Component } from '@angular/core';
import { UserService } from './user-service/user';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { Header } from './header/header';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterModule, Header],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  public showHeader: boolean = false;
  constructor(
    private userService: UserService,
    private router: Router,
  ) {}

  public ngOnInit(): void {
    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      this.showHeader = this.userService.isLoggedInSubject.value;
    });
  }
}
