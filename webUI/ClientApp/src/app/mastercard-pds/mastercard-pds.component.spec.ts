import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MastercardPdsComponent } from './mastercard-pds.component';

describe('MastercardPdsComponent', () => {
  let component: MastercardPdsComponent;
  let fixture: ComponentFixture<MastercardPdsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MastercardPdsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MastercardPdsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
