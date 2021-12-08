import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RemoveCollaborationsDialogComponent } from './remove-collaborations-dialog.component';

describe('RemoveCollaborationsDialogComponent', () => {
  let component: RemoveCollaborationsDialogComponent;
  let fixture: ComponentFixture<RemoveCollaborationsDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RemoveCollaborationsDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RemoveCollaborationsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
