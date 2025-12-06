import { Routes } from '@angular/router';
import { Register } from './register/register';
import { Login } from './login/login';
import { Signup } from './signup/signup';
import { Forum } from './forum/forum';
import { AuthGuard } from './auth.guard';
import { AdminPanel } from './admin-panel/admin-panel';
import { Posts } from './posts/posts';
import { PostDetail } from './post-detail/post-detail';

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
  },
  // SAFE
  {
    path: 'post/:id',
    component: PostDetail,
    data: { vulnerable: false },
    canActivate: [AuthGuard],
  },
  {
    path: 'user/:id/posts',
    component: Posts,
    data: { vulnerable: false },
    canActivate: [AuthGuard],
  },
  // VULNERABLE
  {
    path: 'v/post/:id',
    component: PostDetail,
    data: { vulnerable: true },
    canActivate: [AuthGuard],
  },
  {
    path: 'v/user/:id/posts',
    component: Posts,
    data: { vulnerable: true },
    canActivate: [AuthGuard],
  },
];
