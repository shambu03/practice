import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MastercardsFgComponent } from './mastercards-fg.component';

describe('MastercardsFgComponent', () => {
  let component: MastercardsFgComponent;
  let fixture: ComponentFixture<MastercardsFgComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MastercardsFgComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MastercardsFgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
