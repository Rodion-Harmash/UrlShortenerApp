import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { UrlTableComponent } from './components/url-table/url-table.component';
import { UrlInfoComponent } from './components/url-info/url-info.component';
import { AboutComponent } from './components/about/about.component';

import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';

const routes: Routes = [
  // Redirect root to URLs
  { path: '', redirectTo: 'urls', pathMatch: 'full' },

  // Public pages
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'urls', component: UrlTableComponent },
  { path: 'about', component: AboutComponent },

  // Protected route (details only for authenticated users)
  { path: 'urls/:id', component: UrlInfoComponent, canActivate: [AuthGuard] },

  // Catch-all
  { path: '**', redirectTo: 'urls' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
