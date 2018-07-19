import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import {
  FormGroup,
  FormControl,
  Validators,
  ValidationErrors,
  FormBuilder
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig> = { containerClass: 'theme-dark-blue', maxDate: this.yesterday() };

  constructor(
    private auth: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.createRegistrationForm();
  }

  private yesterday(): Date {
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    return yesterday;
  }

  private createRegistrationForm(): void {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(8),
            Validators.maxLength(30)
          ]
        ],
        confirmPassword: ['', Validators.required]
      },
      { validator: this.passwordMatchValidator }
    );
  }

  private passwordMatchValidator(
    formGroup: FormGroup
  ): ValidationErrors | null {
    return formGroup.get('password').value ===
      formGroup.get('confirmPassword').value
      ? null
      : { passwordMismatch: true };
  }

  register() {
    // this.auth.register(this.model).subscribe(() => {
    //   this.alertify.success('registration complete');
    // }, error => {
    //   this.alertify.error(error);
    // });
    console.log(JSON.stringify(this.registerForm.value));
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
