import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../user-service/user';
import { KENDO_INPUTS } from '@progress/kendo-angular-inputs';
import { KENDO_BUTTONS } from '@progress/kendo-angular-buttons';
import { KENDO_LABELS } from '@progress/kendo-angular-label';

@Component({
  selector: 'app-login',
  imports: [FormsModule, ReactiveFormsModule, CommonModule, KENDO_LABELS, KENDO_INPUTS, KENDO_BUTTONS],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  public useVulnerableLogin: boolean = false;

  constructor(private userService: UserService) {}

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

  public login(): void {
    if (this.loginForm.valid) {
      let formValue = this.loginForm.value;

      if (this.useVulnerableLogin) {
        this.userService.loginVulnerable(formValue.email, formValue.password);
      } else {
        this.userService.login(formValue.email, formValue.password);
      }
      
      this.clearForm();
    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}
