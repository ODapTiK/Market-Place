import { FormControl } from "@angular/forms";

export interface UserRegistrationForm {
    Email: FormControl<string | null>;
    Password: FormControl<string | null>;
    Role: FormControl<'User' | null>;
    Name: FormControl<string | null>;
    Surname: FormControl<string | null>;
    BirthDate: FormControl<string | null>;
}
