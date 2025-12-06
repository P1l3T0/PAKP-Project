import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { KENDO_BUTTONS } from '@progress/kendo-angular-buttons';
import { KENDO_INPUTS } from '@progress/kendo-angular-inputs';
import { KENDO_LABELS } from '@progress/kendo-angular-label';
import { Router } from '@angular/router';
import { SafeHtmlPipe } from "../safehtml-pipe";
import { dataSqlIcon, SVGIcon } from '@progress/kendo-svg-icons';

@Component({
  selector: 'app-forum',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    KENDO_LABELS,
    KENDO_INPUTS,
    KENDO_BUTTONS,
    SafeHtmlPipe
  ],
  templateUrl: './forum.html',
  styleUrl: './forum.css',
})
export class Forum {
  public commentForm: FormGroup = new FormGroup({
    comment: new FormControl(''),
  });

  public comments: any[] = [];
  public vulnerableMode: boolean = false;
  public dataSqlIcon: SVGIcon = dataSqlIcon;

  constructor(
    private router: Router,
  ) {}

  public addComment(): void {
    if (this.commentForm.valid) {
      let comment = this.commentForm.value.comment;

      this.comments.push(comment);
      this.commentForm.reset();
    } else {
      this.commentForm.markAllAsTouched();
    }
  }

  public onChange(): void {
    this.comments = [];
  }

  public adminNavigate(): void {
    this.router.navigate(['/admin-panel']); 
  }
}
