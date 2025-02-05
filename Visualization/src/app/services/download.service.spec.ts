import { TestBed } from '@angular/core/testing';

import { MessageService } from './message.service';
import { HttpClientModule } from '@angular/common/http';
import { DownloadService } from './download.service';

describe('DownloadService', () => {
  let service: DownloadService;

  beforeEach(() => {
      TestBed.configureTestingModule({
        imports: [HttpClientModule],
        providers: [MessageService]
      });
      service = TestBed.inject(DownloadService);
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });
});
