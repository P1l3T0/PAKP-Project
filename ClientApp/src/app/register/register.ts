import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../user-service/user';
import { KENDO_INPUTS } from '@progress/kendo-angular-inputs';
import { KENDO_BUTTONS } from '@progress/kendo-angular-buttons';
import { KENDO_LABELS } from '@progress/kendo-angular-label';

@Component({
  selector: 'app-register',
  imports: [FormsModule, ReactiveFormsModule, CommonModule, KENDO_LABELS, KENDO_INPUTS, KENDO_BUTTONS],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  constructor(private userService: UserService) {}

  public signUpForm: FormGroup = new FormGroup({
    email: new FormControl('', Validators.required),
    username: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });

  public clearForm(): void {
    this.signUpForm.reset();
  }

  public signup(): void {
    if (this.signUpForm.valid) {
      let formValue = this.signUpForm.value;

      this.userService.register(formValue.email, formValue.username, formValue.password);
      
      this.clearForm();
    } else {
      this.signUpForm.markAllAsTouched();
    }
  }
}
