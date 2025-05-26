import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManufacturerLayoutComponent } from './manufacturer-layout.component';

describe('ManufacturerLayoutComponent', () => {
  let component: ManufacturerLayoutComponent;
  let fixture: ComponentFixture<ManufacturerLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManufacturerLayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManufacturerLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
