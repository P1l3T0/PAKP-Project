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
import { KENDO_DIALOG } from '@progress/kendo-angular-dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    KENDO_LABELS,
    KENDO_INPUTS,
    KENDO_BUTTONS,
    KENDO_DIALOG,
  ],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  public showDialog = false;
  public dialogSuccess = false;
  public dialogMessage = '';

  constructor(private userService: UserService, private router: Router) {}

  public registerForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    username: new FormControl('', Validators.required),
    password: new FormControl('', [Validators.required, Validators.minLength(6), Validators.pattern("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{10,}$")]),
  });

  public clearForm(): void {
    this.registerForm.reset();
  }

  public async signup(): Promise<void> {
    if (this.registerForm.valid) {
      let formValue = this.registerForm.value;
      const result = await this.userService.register(
        formValue.email,
        formValue.username,
        formValue.password
      );
      this.dialogSuccess = result.success;
      this.dialogMessage = result.success
        ? 'Registration successful! Would you like to go to login?'
        : 'Registration failed. Please try again.';
      this.showDialog = true;
      this.clearForm();
    } else {
      this.registerForm.markAllAsTouched();
    }
  }

  public onDialogClose(): void {
    this.showDialog = false;
  }

  public goToLogin(): void {
    this.showDialog = false;
    this.router.navigate(['/signup/login']);
  }
}
