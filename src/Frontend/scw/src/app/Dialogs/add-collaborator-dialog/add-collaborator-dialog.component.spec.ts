import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCollaboratorDialogComponent } from './add-collaborator-dialog.component';

describe('AddCollaboratorDialogComponent', () => {
  let component: AddCollaboratorDialogComponent;
  let fixture: ComponentFixture<AddCollaboratorDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddCollaboratorDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCollaboratorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
