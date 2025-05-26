import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminNotificationPageComponent } from './admin-notification-page.component';

describe('AdminNotificationPageComponent', () => {
  let component: AdminNotificationPageComponent;
  let fixture: ComponentFixture<AdminNotificationPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminNotificationPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminNotificationPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
