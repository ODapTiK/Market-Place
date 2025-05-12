import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManufacturerProfilePageComponent } from './manufacturer-profile-page.component';

describe('ManufacturerProfilePageComponent', () => {
  let component: ManufacturerProfilePageComponent;
  let fixture: ComponentFixture<ManufacturerProfilePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManufacturerProfilePageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManufacturerProfilePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
