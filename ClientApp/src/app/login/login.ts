import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { UserService } from '../user-service/user';
import { KENDO_INPUTS } from '@progress/kendo-angular-inputs';
import { KENDO_BUTTONS } from '@progress/kendo-angular-buttons';
import { KENDO_LABELS } from '@progress/kendo-angular-label';
import { Router } from '@angular/router';
import { KENDO_DIALOG } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'app-login',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    KENDO_LABELS,
    KENDO_INPUTS,
    KENDO_BUTTONS,
    KENDO_DIALOG
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  public useVulnerableLogin: boolean = false;
  public showDialog = false;
  public dialogMessage = '';
  public dialogTitle = 'Login not successful!';

  constructor(private userService: UserService, private router: Router) {}

  public loginForm: FormGroup = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });

  public clearForm(): void {
    this.loginForm.reset();
  }

  public toggleLoginMethod(): void {
    this.useVulnerableLogin = !this.useVulnerableLogin;
  }

  public async login(): Promise<void> {
    if (this.loginForm.valid) {
      const formValue = this.loginForm.value;
      const result = await this.userService.login(formValue.email, formValue.password, this.useVulnerableLogin);

      if (result.success) {
        this.router.navigate(['/forum']);
      } else {
        this.showDialog = true;
        this.dialogMessage = result.message || 'Login failed. Please try again.';
      }

      this.clearForm();
    } else {
      this.loginForm.markAllAsTouched();
    }
  }

  public onDialogClose(): void {
    this.showDialog = false;
  }
}
