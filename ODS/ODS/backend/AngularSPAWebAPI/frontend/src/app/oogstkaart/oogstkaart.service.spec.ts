import { TestBed, inject } from '@angular/core/testing';

import { OogstkaartService } from './oogstkaart.service';

describe('OogstkaartService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [OogstkaartService]
    });
  });

  it('should be created', inject([OogstkaartService], (service: OogstkaartService) => {
    expect(service).toBeTruthy();
  }));
});
