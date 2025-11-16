import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { KENDO_LAYOUT, SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';
import { ActivatedRoute, NavigationEnd, Router, RouterModule } from "@angular/router";
import { filter, startWith } from 'rxjs';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, KENDO_LAYOUT, RouterModule],
  templateUrl: './signup.html',
  styleUrl: './signup.css',
})
export class Signup {
  @ViewChild('tabstrip') public tabstrip!: TabStripComponent;
  constructor(public router: Router, private route: ActivatedRoute, private cdr: ChangeDetectorRef) { }
  
  public ngAfterViewInit(): void {
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        startWith(this.router)
      )
      .subscribe((event: any) => {
        this.manageTabsSelection(this.router.url);
        this.cdr.detectChanges();
      })
  }

  public onTabSelect(e: SelectEvent): void {
    if (e.title == 'Log in') {
      this.router.navigate(['login'], { relativeTo: this.route });
    } else {
      this.router.navigate(['register'], { relativeTo: this.route });
    }
  }

   public manageTabsSelection(url: any): void {
    if (url.includes('login')) {
      this.tabstrip.selectTab(0);
    } else {
      this.tabstrip.selectTab(1);
    }
  }
}
