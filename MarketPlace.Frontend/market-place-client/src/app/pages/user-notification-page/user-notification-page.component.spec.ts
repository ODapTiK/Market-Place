import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserNotificationPageComponent } from './user-notification-page.component';

describe('UserNotificationPageComponent', () => {
  let component: UserNotificationPageComponent;
  let fixture: ComponentFixture<UserNotificationPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserNotificationPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserNotificationPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
