import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpreadjsComponent } from './spreadjs.component';

describe('SpreadjsComponent', () => {
  let component: SpreadjsComponent;
  let fixture: ComponentFixture<SpreadjsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SpreadjsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SpreadjsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
