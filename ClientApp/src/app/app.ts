import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from './user-service/user';
import { eyeIcon, facebookIcon, googleIcon, linkedinIcon, SVGIcon } from "@progress/kendo-svg-icons";
import { KENDO_INPUTS } from '@progress/kendo-angular-inputs';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {}
