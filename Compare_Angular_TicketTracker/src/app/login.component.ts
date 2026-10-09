import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { errorMessage } from './api-error';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  submitted = false;
  busy = false;
  error = '';

  form = inject(FormBuilder).nonNullable.group({
    username: ['demo', Validators.required],
    password: ['', Validators.required],
  });

  submit() {
    this.submitted = true;
    this.error = '';
    if (this.form.invalid) return;
    this.busy = true;
    const { username, password } = this.form.getRawValue();
    this.auth.login(username.trim(), password).subscribe({
      next: () => {
        const returnUrl = this.router.parseUrl(this.router.url).queryParams['returnUrl'] ?? '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: (e) => {
        this.error = e.status === 401 ? 'Benutzername oder Passwort falsch.' : errorMessage(e);
        this.busy = false;
      },
    });
  }
}
