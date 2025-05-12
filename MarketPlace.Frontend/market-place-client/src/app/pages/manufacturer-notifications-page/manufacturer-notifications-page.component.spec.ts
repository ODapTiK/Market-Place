import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManufacturerNotificationsPageComponent } from './manufacturer-notifications-page.component';

describe('ManufacturerNotificationsPageComponent', () => {
  let component: ManufacturerNotificationsPageComponent;
  let fixture: ComponentFixture<ManufacturerNotificationsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManufacturerNotificationsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManufacturerNotificationsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
