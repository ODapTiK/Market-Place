import { FormControl } from "@angular/forms";

export interface ManufacturerRegistrationForm {
    Email: FormControl<string | null>;
    Password: FormControl<string | null>;
    Role: FormControl<'Manufacturer' | null>;
    Organization: FormControl<string | null>;
}
