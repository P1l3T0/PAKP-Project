import { Component, signal } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from './user-service/user';

@Component({
  selector: 'app-root',
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
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
