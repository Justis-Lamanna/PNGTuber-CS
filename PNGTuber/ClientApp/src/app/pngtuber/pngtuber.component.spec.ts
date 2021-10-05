import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PNGTuberComponent } from './pngtuber.component';

describe('PNGTuberComponent', () => {
  let component: PNGTuberComponent;
  let fixture: ComponentFixture<PNGTuberComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PNGTuberComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PNGTuberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
