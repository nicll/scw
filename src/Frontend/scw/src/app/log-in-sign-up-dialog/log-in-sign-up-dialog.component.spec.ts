import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogInSignUpDialogComponent } from './log-in-sign-up-dialog.component';

describe('LogInSignUpDialogComponent', () => {
  let component: LogInSignUpDialogComponent;
  let fixture: ComponentFixture<LogInSignUpDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LogInSignUpDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LogInSignUpDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
