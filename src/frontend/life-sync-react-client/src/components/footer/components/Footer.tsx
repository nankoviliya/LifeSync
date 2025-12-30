import { ExternalLink, Github, Linkedin } from 'lucide-react';

export const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-muted py-2.5 text-center text-muted-foreground transition-colors">
      <div className="mx-auto max-w-[1200px]">
        <p className="my-2.5 text-sm">
          &copy; {currentYear} nankoviliya. All Rights Reserved.
        </p>
        <div className="mt-2.5">
          <a
            href="https://google.com"
            target="_blank"
            rel="noopener noreferrer"
            className="mx-4 inline-flex items-center gap-1 text-muted-foreground no-underline transition-colors hover:text-foreground focus:outline-none"
          >
            Portfolio
            <ExternalLink className="h-4 w-4" />
          </a>
          <a
            href="https://github.com/nankoviliya"
            target="_blank"
            rel="noopener noreferrer"
            className="mx-4 inline-flex items-center gap-1 text-muted-foreground no-underline transition-colors hover:text-foreground focus:outline-none"
          >
            GitHub
            <Github className="h-4 w-4" />
          </a>
          <a
            href="https://www.linkedin.com/in/iliya-nankov/"
            target="_blank"
            rel="noopener noreferrer"
            className="mx-4 inline-flex items-center gap-1 text-muted-foreground no-underline transition-colors hover:text-foreground focus:outline-none"
          >
            LinkedIn
            <Linkedin className="h-4 w-4" />
          </a>
        </div>
      </div>
    </footer>
  );
};
