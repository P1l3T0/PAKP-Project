import { Routes } from '@angular/router';
import { Register } from './register/register';
import { Login } from './login/login';
import { Signup } from './signup/signup';
import { Forum } from './forum/forum';
import { AuthGuard } from './auth.guard';
import { AdminPanel } from './admin-panel/admin-panel';

export const routes: Routes = [
  { path: '', redirectTo: 'signup', pathMatch: 'full' },
  {
    path: 'signup',
    component: Signup,
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      {
        path: 'register',
        component: Register,
      },
      {
        path: 'login',
        component: Login,
      },
    ],
  },
  {
    path: 'forum',
    component: Forum,
    canActivate: [AuthGuard],
  },
  {
    path: 'admin-panel',
    component: AdminPanel,
    canActivate: [AuthGuard],
  }
];